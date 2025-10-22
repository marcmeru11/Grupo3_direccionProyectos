import os
import random
from PIL import Image

# Ruta a la carpeta que contiene imágenes PNG y JSON (puede ser relativa o absoluta)
ruta = 'Backend/tensor/dataset/cropped_images'  # Ajusta según tu estructura de carpetas

# Listar todos los archivos PNG en la carpeta
todas_imagenes_nombres = [f for f in os.listdir(ruta) if f.endswith('.png')]

# Listar todos los archivos JSON en la carpeta
jsons_nombres = [f for f in os.listdir(ruta) if f.endswith('.json')]

# Filtrar imágenes "buenas" que no tengan JSON asociado
imagenes_buenas_nombres = []
for img in todas_imagenes_nombres:
    json_name = os.path.splitext(img)[0] + '.json'
    if json_name not in jsons_nombres:
        imagenes_buenas_nombres.append(img)

# Cargar todas las imágenes (buenas y malas) en una lista
datos_todas_imagenes = []
for img in todas_imagenes_nombres:
    path_img = os.path.join(ruta, img)
    imagen = Image.open(path_img)
    datos_todas_imagenes.append(imagen)

# Cargar solo imágenes buenas en otra lista
datos_imagenes_buenas = []
for img in imagenes_buenas_nombres:
    path_img = os.path.join(ruta, img)
    imagen = Image.open(path_img)
    datos_imagenes_buenas.append(imagen)

# Mezclar aleatoriamente las imágenes buenas
random.shuffle(datos_imagenes_buenas)

# Dividir 80% entrenamiento, 20% validación de imágenes buenas
num_entrenamiento = int(len(datos_imagenes_buenas) * 0.8)

train_data = datos_imagenes_buenas[:num_entrenamiento]
val_data = datos_imagenes_buenas[num_entrenamiento:]

# Resultado:
# datos_todas_imagenes -> todas las imágenes (buenas y malas)
# train_data -> 80% imágenes buenas para entrenamiento
# val_data -> 20% imágenes buenas para validación limpia

print(f"Total imágenes: {len(datos_todas_imagenes)}")
print(f"Imágenes buenas: {len(datos_imagenes_buenas)}")
print(f"Entrenamiento: {len(train_data)}")
print(f"Validación limpia: {len(val_data)}")
