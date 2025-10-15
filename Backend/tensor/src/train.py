import data_loader as dt
import tensorflow as tf
import numpy as np

img_height = 500
img_width = 464
channels = 3

train_ds = dt.get_train_dataset()
val_ds = dt.get_validation_set()
val_ds_correct = dt.get_validation_set_correct()

inputs = tf.keras.layers.Input(shape=(img_height, img_width, channels))

#"Comprime" las imagenes para procesarlas mejor
x = tf.keras.layers.Conv2D(32, 3, activation='relu', padding='same')(inputs)
x = tf.keras.layers.MaxPooling2D()(x)
x = tf.keras.layers.Conv2D(64, 3, activation='relu', padding='same')(x)
encoded = tf.keras.layers.MaxPooling2D()(x)

#"Descomprime" las imagenes de vuelta
x = tf.keras.layers.Conv2DTranspose(64, 3, strides=2, activation='relu', padding='same')(encoded)
x = tf.keras.layers.Conv2DTranspose(32, 3, strides=2, activation='relu', padding='same')(x)
decoded = tf.keras.layers.Conv2D(channels, 3, activation='sigmoid', padding='same')(x)

#Se crea el modelo que sirve para analizar las imagenes
autoencoder = models.Model(inputs, decoded)
autoencoder.compile(optimizer='adam', loss='mse')

train_ds_auto = train_ds.map(lambda x: (x, x))
val_ds_auto = val_ds.map(lambda x: (x, x))

#Entrena el modelo con los dataset dados
autoencoder.fit(
    train_ds_auto,
    validation_data=val_ds_auto,
    epochs=2
)

#Calculamos el threshold con los errores de un set de validacion con solo imagenes correctas
def calculate_threshold(ds):
    errors = []

    for batch in ds:
        x, _ = batch 
        reconstructed = autoencoder.predict(x)
        batch_errors = np.mean(np.square(x.numpy() - reconstructed), axis=(1,2,3))  
        errors.extend(batch_errors)

    errors = np.array(errors)
    threshold = np.mean(errors) + 3*np.std(errors) 

calculate_threshold(val_ds_correct)

autoencoder.save("autoencoder_model")

def is_anomaly(img, model, threshold):
    img = tf.expand_dims(img, 0)  #añadir batch
    reconstructed = model.predict(img) #calcular salida
    error = tf.reduce_mean(tf.square(img - reconstructed)).numpy()  #calcular el error
    return error > threshold, error #Si el error el mas que el threshold, imagen mala

#TODO revisar la manera de comprobar si es una anomalia y reestructurar el codigo para que sea mas legible y probar que funciona completamente