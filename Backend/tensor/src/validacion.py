import tensorflow as tf
import numpy as np
import json
import data_loader as dt
import sys
import os

#Configuración general
MODEL_PATH = "Backend/tensor/src/model.keras"
METADATA_PATH = "Backend/tensor/src/model/metadata.json"
OUTPUT_PATH = "Backend/tensor/src/results.json"

IMG_HEIGHT = 500
IMG_WIDTH = 464

#Cargamos el modelo y el threshold calculado
print("Cargando modelo y metadata...")
model = tf.keras.models.load_model(MODEL_PATH, compile=False)
with open(METADATA_PATH, "r") as f:
    threshold = json.load(f)["threshold"]

#Normalizamos la(s) imagen(es) insertadas
def load_image(path):
    img = tf.io.read_file(path)
    img = tf.image.decode_png(img, channels=3)
    img = tf.image.resize(img, [IMG_HEIGHT, IMG_WIDTH])
    img = img / 255.0
    return img

#Calculamenos el error para una sola imagen
def evaluate_single_image(model, threshold, img_path):
    img = load_image(img_path)
    img = tf.expand_dims(img, 0)
    reconstructed = model.predict(img, verbose=0)
    error = np.mean(np.abs(img.numpy() - reconstructed))
    is_defective = error > threshold

    status = "DEFECTIVE" if is_defective else "NORMAL"
    print(f"\nResultado para '{img_path}': {status} | Error = {error:.6f}")

    return {
        "image_path": img_path,
        "error": float(error),
        "status": status.lower()
    }

#Calculamos el error de cada una de las imagenes de un directorio
def evaluate_dataset(model, threshold, dataset, output_path=OUTPUT_PATH):
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
                "image_path": file_path,
                "error": float(error),
                "status": status
            })
            print(f"[{index:04d}] {status.upper()} | Error = {error:.6f}")
            index += 1

    with open(output_path, "w") as f:
        json.dump(results, f, indent=2)

    print(f"\nResultados guardados en {output_path}")
    return results