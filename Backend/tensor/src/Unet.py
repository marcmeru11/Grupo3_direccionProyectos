"""
U-Net Style Autoencoder for Anomaly Detection
Process ALL validation images
"""

import tensorflow as tf
import numpy as np
import json
import data_loader as dt

#Variables generales de las imagenes
IMG_HEIGHT = 500
IMG_WIDTH = 464
CHANNELS = 3
EPOCHS = 20

#Cargamos datasets desde la clase data_loader
train_ds = dt.get_train_dataset()
val_ds = dt.get_validation_set()
val_ds_correct = dt.get_validation_set_correct()

#Cargamos los datasets "duplicados" para tener las imagenes orginales y las reconstruidas
train_ds_auto = train_ds.map(lambda path, img: (img, img))
val_ds_auto = val_ds.map(lambda path, img: (img, img))

#Reducimos la resolucion de las imagenes para extraer caracteristicas
def encoder_block(inputs, num_filters, dropout=False):
    x = tf.keras.layers.Conv2D(num_filters, 3, activation='relu', padding='same')(inputs)
    x = tf.keras.layers.MaxPooling2D()(x)
    if dropout:
        x = tf.keras.layers.Dropout(0.2)(x)
    return x

#Reconstruimos la imagen mediante upsampling
def decoder_block(inputs, num_filters):
    x = tf.keras.layers.Conv2DTranspose(num_filters, 3, strides=2, activation='relu', padding='same')(inputs)
    return x

#Creamos el modelo basado en el encoder y decoder anterior
def autoencoder_model(input_shape=(IMG_HEIGHT, IMG_WIDTH, CHANNELS)):
    inputs = tf.keras.layers.Input(shape=input_shape)

    e1 = encoder_block(inputs, 8)
    e2 = encoder_block(e1, 16, dropout=True)
    b = tf.keras.layers.Conv2D(32, 3, activation='relu', padding='same')(e2)
    d1 = decoder_block(b, 16)
    d2 = decoder_block(d1, 8)
    outputs = tf.keras.layers.Conv2D(CHANNELS, 3, activation='sigmoid', padding='same')(d2)

    return tf.keras.Model(inputs, outputs, name='Autoencoder_UNetStyle')

#Calculamos el MAE entre la imagen original y la reconstruida por el autoencoder
def reconstruction_error(original, reconstructed):
    return np.mean(np.abs(original - reconstructed))

#Comprobamos si es una anomalia si el error es mayor al threshold
def is_anomaly(img, model, threshold):
    img = tf.expand_dims(img, 0)
    reconstructed = model.predict(img, verbose=0)
    error = reconstruction_error(img.numpy(), reconstructed)
    return error > threshold, error

#Calculamos el threshold como el percentil del error de reconstrucción en imágenes buenas
def calculate_threshold(model, ds_correct, percentile=90):
    errors = []
    for _, batch in ds_correct:
        reconstructed = model.predict(batch, verbose=0)
        batch_errors = np.mean(np.abs(batch.numpy() - reconstructed), axis=(1, 2, 3))
        errors.extend(batch_errors)
    return np.percentile(errors, percentile)

#Entrenamos el autoencoder con EarlyStopping y guardado del modelo en "model.keras"
def train_autoencoder():
    model = autoencoder_model()
    model.compile(optimizer='adam', loss='mse', metrics=['mae'])
    callback = tf.keras.callbacks.EarlyStopping(monitor="val_loss", patience=3, restore_best_weights=True)

    model.fit(train_ds_auto, validation_data=val_ds_auto, epochs=EPOCHS, callbacks=[callback])

    threshold = calculate_threshold(model, val_ds_correct)

    model.save("model.keras")
    metadata = {"threshold": float(threshold)}
    with open("model/metadata.json", "w") as f:
        json.dump(metadata, f)

    return model, threshold
'''
#Evaluamos todas las imágenes del dataset y genera un JSON con los errores y clasificaciones
def evaluate_all_images(model, threshold, dataset, output_path="results.json"):
    results = []
    index = 0

    for paths, batch in dataset:
        reconstructed = model.predict(batch, verbose=0)
        batch_errors = np.mean(np.abs(batch.numpy() - reconstructed), axis=(1, 2, 3))

        for i, error in enumerate(batch_errors):
            file_path = paths[i].numpy().decode("utf-8")
            status = "defective" if error > threshold else "normal"
            results.append({
                "image_index": index,
                "error": float(error),
                "status": status
            })
            print(f"[{index:04d}] {status.upper()} | Error = {error:.6f}")
            index += 1

    with open(output_path, "w") as f:
        json.dump(results, f, indent=2)

    return results
'''

if __name__ == '__main__':
    mode, threshold = train_autoencoder()