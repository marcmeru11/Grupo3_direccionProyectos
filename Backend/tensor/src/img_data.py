import os
import random
from PIL import Image

# Ruta a la carpeta que contiene imágenes PNG y JSON (puede ser relativa o absoluta)
ruta = 'Backend/tensor/dataset/cropped_images'  # Ajusta según tu estructura de carpetas

archivos = os.listdir(ruta)
imagenes = [f for f in archivos if f.endswith('.png')]
jsons = [f for f in archivos if f.endswith('.json')]

# Separar imágenes buenas y malas por nombre
imagenes_buenas = []
imagenes_malas = []
datos_todas_imagenes = []

for img in imagenes:

    # Cargar la imagen para la variable de todas
    path_img = os.path.join(ruta, img)
    imagen = Image.open(path_img)
    datos_todas_imagenes.append(imagen)

    json_name = os.path.splitext(img)[0] + '.json'
    if json_name in jsons:
        imagenes_malas.append(img)
    else:
        imagenes_buenas.append(img)

# Cargar solo imágenes buenas
datos_imagenes_buenas = []
for img in imagenes_buenas:
    path_img = os.path.join(ruta, img)
    datos_imagenes_buenas.append(img)  # Puedes guardar nombres o cargar imágenes según tu pipeline

# Mezclar, dividir en 80/20
random.shuffle(datos_imagenes_buenas)
num_entrenamiento = int(len(datos_imagenes_buenas) * 0.8)
train_data = datos_imagenes_buenas[:num_entrenamiento]
val_nombres = datos_imagenes_buenas[num_entrenamiento:]

# Cargar solo imágenes malas
datos_imagenes_malas = []
for img in imagenes_malas:
    path_img = os.path.join(ruta, img)
    datos_imagenes_malas.append(img)

# Unión de malas y el 20% buenas para validación final
val_final_nombres = val_nombres + datos_imagenes_malas

# Puedes cargar todas como imágenes si lo prefieres:
val_data = []
for img in val_final_nombres:
    path_img = os.path.join(ruta, img)
    imagen = Image.open(path_img)
    val_data.append(imagen)

# Resultado:
# datos_todas_imagenes -> todas las imágenes (buenas y malas)
# train_data -> 80% imágenes buenas para entrenamiento
# val_data -> 20% imágenes buenas para validación limpia

print(f"Total imágenes: {len(datos_todas_imagenes)}")
print(f"Imágenes buenas: {len(datos_imagenes_buenas)}")
print(f"Entrenamiento: {len(train_data)}")
print(f"Validación limpia: {len(val_data)}")
