import data_loader as dt
import tensorflow as tf
import numpy as np
import json

img_height = 500
img_width = 464
channels = 3

train_ds = dt.get_train_dataset()
val_ds = dt.get_validation_set()
val_ds_correct = dt.get_validation_set_correct()

inputs = tf.keras.layers.Input(shape=(img_height, img_width, channels))

#"Comprime" las imagenes para procesarlas mejor
x = tf.keras.layers.Conv2D(8, 3, activation='relu', padding='same')(inputs)
x = tf.keras.layers.MaxPooling2D()(x)
x = tf.keras.layers.Conv2D(16, 3, activation='relu', padding='same')(x)
encoded = tf.keras.layers.MaxPooling2D()(x)
encoded = tf.keras.layers.Dropout(0.2)(encoded)


#"Descomprime" las imagenes de vuelta
x = tf.keras.layers.Conv2DTranspose(16, 3, strides=2, activation='relu', padding='same')(encoded)
x = tf.keras.layers.Conv2DTranspose(8, 3, strides=2, activation='relu', padding='same')(x)
decoded = tf.keras.layers.Conv2D(channels, 3, activation='sigmoid', padding='same')(x)

""" 
    Crear modelo
"""
num_classes = 2

def is_anomaly(img, autoencoder, threshold):
    img = tf.expand_dims(img, 0)  #añadir batch
    reconstructed = autoencoder.predict(img) #calcular salida
    error = tf.reduce_mean(tf.abs(img - reconstructed)).numpy()  # MAE
    return error > threshold, error #Si el error el mas que el threshold, imagen mala

train_ds_auto = train_ds.map(lambda x: (x, x))
val_ds_auto = val_ds.map(lambda x: (x, x))

autoencoder = tf.keras.Model(inputs, decoded)

"""
    loss: mse (minimum square error)
"""
epochs = 20
autoencoder.compile(
    optimizer = 'adam', 
    loss = 'mse',
    metrics = ['mae'])
"""
    Entrena el modelo con los dataset dados
    epochs: numero de iteraciones
"""
callback = tf.keras.callbacks.EarlyStopping(monitor="val_loss", patience=3, restore_best_weights=True)
autoencoder.fit(train_ds_auto, validation_data = val_ds_auto, epochs = epochs, callbacks=[callback])

#Calculamos el threshold con los errores de un set de validacion con solo imagenes correctas
def calculate_threshold(ds):
    errors = []

    for batch in ds:
        x = batch
        reconstructed = autoencoder.predict(x)
        batch_errors = np.mean(np.abs(x.numpy() - reconstructed), axis=(1,2,3))  # MAE
        errors.extend(batch_errors)

    errors = np.array(errors)
    return np.percentile(errors, 90)

threshold = calculate_threshold(val_ds_correct)

"""
    calcular metricas
"""
for batch in val_ds.take(1):
    for i in range(batch.shape[0]):
        flag, error = is_anomaly(batch[i], autoencoder, threshold)
        print("anomalia" if flag else "normal", "Error: ", error)

autoencoder.save("model.keras")

print(type(autoencoder))

metadata = {"threshold": float(threshold)}

with open("model/metadata.json", "w") as file:
    json.dump(metadata, file)

#TODO revisar la manera de comprobar si es una anomalia y reestructurar el codigo para que sea mas legible y probar que funciona completamente
