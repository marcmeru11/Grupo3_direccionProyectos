import os
import random
import tensorflow as tf

##Preprocesado de directorio de imagenes

#Ruta donde se encuentran todas las imágenes y sus json
ruta = 'Backend/tensor/dataset/cropped_images'

#Separamos en dos grupos las imagenes y los json
archivos = os.listdir(ruta)
imagenes = [f for f in archivos if f.endswith('.png')]
jsons = [f for f in archivos if f.endswith('.json')]

#hacemos las listas que contendran las imagenes separadas
imagenes_buenas = []
imagenes_malas = []
datos_todas_imagenes = []

#Separamos las imagenes buenas de las imagenes con anomalias
for img in imagenes:
    path_img = os.path.join(ruta, img)
    datos_todas_imagenes.append(path_img)

    json_name = os.path.splitext(img)[0] + '.json'
    if json_name in jsons:
        imagenes_malas.append(path_img)
    else:
        imagenes_buenas.append(path_img)

#Mezclamos y dividimos las imagenes buenas 80% y 20%
random.shuffle(imagenes_buenas)
num_entrenamiento = int(len(imagenes_buenas) * 0.8)
train_data = imagenes_buenas[:num_entrenamiento]
val_nombres = imagenes_buenas[num_entrenamiento:]

#Juntamos las imagenes malas en la validacion, quedando 20% buenas 80% (todas) malas
val_data = val_nombres + imagenes_malas

##Procesado de los datasets
#Variables generales para las imagenes
IMG_HEIGHT = 500
IMG_WIDTH = 464
BATCH_SIZE = 8

#Normalizacion de las imagenes
def preprocess_image(path):
    img = tf.io.read_file(path)
    img = tf.image.decode_png(img, channels=3)
    img = tf.image.resize(img, [IMG_HEIGHT, IMG_WIDTH])
    img = tf.cast(img, tf.float32) / 255.0
    return img

#Convertimos los paths en datasets de tensor, guardando el path y la imagen de cada una de ellas
def paths_to_dataset(paths):
    ds = tf.data.Dataset.from_tensor_slices(paths)
    ds = ds.map(
        lambda p: (p, preprocess_image(p)),
        num_parallel_calls=tf.data.AUTOTUNE
    )
    ds = ds.batch(BATCH_SIZE).prefetch(tf.data.AUTOTUNE)
    return ds

#Funciones para devolver los distinitos datasets
def get_train_dataset():
    return paths_to_dataset(train_data)

def get_validation_set():
    return paths_to_dataset(val_data)

def get_validation_set_correct():
    return paths_to_dataset(val_nombres)

def get_dataset():
    return paths_to_dataset(datos_todas_imagenes)

#Prints para comprobar la cantidad de imagenes de cada dataset
print(f"Total imágenes: {len(datos_todas_imagenes)}")
print(f"Imágenes buenas: {len(imagenes_buenas)}")
print(f"Entrenamiento: {len(train_data)}")
print(f"Validación final: {len(val_data)}")
