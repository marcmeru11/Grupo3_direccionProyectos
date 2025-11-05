"""
U-Net Style Autoencoder for Anomaly Detection
Process ALL validation images
"""

import tensorflow as tf
import numpy as np
import json
import data_loader as dt

IMG_HEIGHT = 500
IMG_WIDTH = 464
CHANNELS = 3
EPOCHS = 20

train_ds = dt.get_train_dataset()
val_ds = dt.get_validation_set()
val_ds_correct = dt.get_validation_set_correct()

train_ds_auto = train_ds.map(lambda x: (x, x))
val_ds_auto = val_ds.map(lambda x: (x, x))

def encoder_block(inputs, num_filters, dropout=False):
    x = tf.keras.layers.Conv2D(num_filters, 3, activation='relu', padding='same')(inputs)
    x = tf.keras.layers.MaxPooling2D()(x)
    if dropout:
        x = tf.keras.layers.Dropout(0.2)(x)
    return x

def decoder_block(inputs, num_filters):
    x = tf.keras.layers.Conv2DTranspose(num_filters, 3, strides=2, activation='relu', padding='same')(inputs)
    return x

def autoencoder_model(input_shape=(IMG_HEIGHT, IMG_WIDTH, CHANNELS)):
    inputs = tf.keras.layers.Input(shape=input_shape)

    e1 = encoder_block(inputs, 8)
    e2 = encoder_block(e1, 16, dropout=True)
    b = tf.keras.layers.Conv2D(32, 3, activation='relu', padding='same')(e2)
    d1 = decoder_block(b, 16)
    d2 = decoder_block(d1, 8)
    outputs = tf.keras.layers.Conv2D(CHANNELS, 3, activation='sigmoid', padding='same')(d2)

    return tf.keras.Model(inputs, outputs, name='Autoencoder_UNetStyle')

"""
    Calcula el error medio absoluto (MAE) entre imagen original y reconstruida.
"""
def reconstruction_error(original, reconstructed):
    return np.mean(np.abs(original - reconstructed))

def is_anomaly(img, model, threshold):
    img = tf.expand_dims(img, 0)
    reconstructed = model.predict(img, verbose=0)
    error = reconstruction_error(img.numpy(), reconstructed)
    return error > threshold, error

def calculate_threshold(model, ds_correct, percentile=90):
    errors = []
    for batch in ds_correct:
        reconstructed = model.predict(batch, verbose=0)
        batch_errors = np.mean(np.abs(batch.numpy() - reconstructed), axis=(1, 2, 3))
        errors.extend(batch_errors)
    return np.percentile(errors, percentile)

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

"""
    Evalúa todas las imágenes del dataset.
    Guarda un archivo JSON con el error y si es anomalía o no.
"""
def evaluate_all_images(model, threshold, dataset, output_path="results.json"):
    results = []
    index = 0

    for batch in dataset:
        reconstructed = model.predict(batch, verbose=0)
        batch_errors = np.mean(np.abs(batch.numpy() - reconstructed), axis=(1, 2, 3))

        for i, error in enumerate(batch_errors):
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