import tensorflow as tf
import pathlib
import matplotlib.pyplot as plt
import img_data as data

"""
    batch: tamaño del conjunto de imagenes que se van a usar
"""

AUTOTUNE = tf.data.AUTOTUNE

#Declaramos variables fijas sobre las fotos
batch_size = 32
img_height = 500
img_width = 464

good_val_path = data.get_val_good_path()
train_path = data.get_train_path()
val_path = data.get_val_path()
#buscamos y creamos un "dataset" inicial desde las imagenes

train_ds = tf.data.Dataset.from_tensor_slices(train_path)
val_ds = tf.data.Dataset.from_tensor_slices(val_path)
good_val_ds = tf.data.Dataset.from_tensor_slices(good_val_path)

#Aplicamos variacion a las fotos para evitar overfitting
data_augmentation = tf.keras.Sequential([
    tf.keras.layers.RandomFlip("horizontal"),
    tf.keras.layers.RandomRotation(0.1),
    tf.keras.layers.RandomZoom(0.1),
])

#Cargamos las imagenes sin augment (variaciones) para validacion
def load_image_no_aug(path):
    img = tf.io.read_file(path)
    img = tf.image.decode_png(img, channels=3)
    img = tf.image.resize(img, [img_height, img_width])
    img = img / 255.0
    return img

#Cargamos las imagenes con augment (variaciones) para entrenamiento
def load_image(path):
    img = tf.io.read_file(path)
    img = tf.image.decode_png(img, channels=3)
    img = tf.image.resize(img, [img_height, img_width])
    img = img / 255.0
    img = data_augmentation(img)
    return img

#Creamos el dataset de entrenamiento con toda la configuracion
train_ds = (
    train_ds
    .map(load_image, num_parallel_calls=AUTOTUNE)
    .cache() #cachea las imagenes para mayor velocidad
    .shuffle(1000) #mezcla las imagenes
    .batch(batch_size) #settea la cantidad de imagenes que se usan de golpe
    .prefetch(AUTOTUNE) #va cargando el siguiente batch
)

#Creamos el dataset de validacion con toda la configuracion
val_ds = (
    val_ds
    .map(load_image_no_aug, num_parallel_calls = AUTOTUNE)
    .cache() #cachea las imagenes para mayor velocidad
    .batch(batch_size) #settea la cantidad de imagenes que se usan de golpe
    .prefetch(AUTOTUNE) #va cargando el siguiente batch
)

#Creamos el dataset de validacion correcta con toda la configuracion
good_val_ds = (
    good_val_ds
    .map(load_image_no_aug, num_parallel_calls = AUTOTUNE)
    .cache() #cachea las imagenes para mayor velocidad
    .batch(batch_size) #settea la cantidad de imagenes que se usan de golpe
    .prefetch(AUTOTUNE) #va cargando el siguiente batch
)

print("Train batches:", sum(1 for _ in train_ds))
print("Validation batches:", sum(1 for _ in val_ds))

def get_train_dataset(): 
    return train_ds

def get_validation_set():
    return val_ds

def get_validation_set_correct():
    return good_val_ds