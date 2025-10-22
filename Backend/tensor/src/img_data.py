import os
import random

# Ruta a la carpeta de datos
ruta = 'Backend/tensor/dataset/cropped_images'  # Ajusta a tu estructura

archivos = os.listdir(ruta)
imagenes = [f for f in archivos if f.endswith('.png')]
jsons = [f for f in archivos if f.endswith('.json')]

# Listas para nombres y rutas
imagenes_buenas = []
imagenes_malas = []
datos_todas_imagenes = []

for img in imagenes:
    # Guardar ruta completa
    path_img = os.path.join(ruta, img)
    datos_todas_imagenes.append(path_img)

    json_name = os.path.splitext(img)[0] + '.json'
    if json_name in jsons:
        imagenes_malas.append(path_img)
    else:
        imagenes_buenas.append(path_img)

# Mezclar y dividir buenas en 80/20
random.shuffle(imagenes_buenas)
num_entrenamiento = int(len(imagenes_buenas) * 0.8)
train_data = imagenes_buenas[:num_entrenamiento]
val_nombres = imagenes_buenas[num_entrenamiento:]

# Validación final: 20% buenas + todas malas
val_data  = val_nombres + imagenes_malas

# Resultado:
# datos_todas_imagenes -> rutas de todas las imágenes
# train_data -> rutas del 80% buenas para entrenamiento
# val_data  -> rutas del 20% buenas + malas para validación final

def get_val_good_path():
    return imagenes_buenas

def get_train_path():
    return train_data

def get_val_path():
    return val_data

print(f"Total imágenes: {len(datos_todas_imagenes)}")
print(f"Imágenes buenas: {len(imagenes_buenas)}")
print(f"Entrenamiento: {len(train_data)}")
print(f"Validación final: {len(val_data )}")
