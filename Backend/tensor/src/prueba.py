import tensorflow as tf
from tensorflow import keras
import numpy as np
import json
import os

# Cargar modelo y threshold
autoencoder = keras.models.load_model("model.keras")

img_height = 500
img_width = 464

with open("model/metadata.json", "r") as f:
    threshold = json.load(f)["threshold"]

def load_image(path):
    img = tf.io.read_file(path)
    img = tf.image.decode_png(img, channels=3)
    img = tf.image.resize(img, [img_height, img_width])
    img = img / 255.0
    return img

def is_anomaly(img, autoencoder, threshold):
    img = tf.expand_dims(img, 0)
    reconstructed = autoencoder.predict(img, verbose=0)
    error = np.mean(np.abs(img.numpy() - reconstructed)) 
    return error > threshold, error

def check_directory(directory, autoencoder, threshold):
    """
    Recorre todas las imágenes del directorio dado,
    las evalúa y muestra si son normales o anómalas.
    """
    files = [f for f in os.listdir(directory) if f.lower().endswith(".png")]

    total = len(files)
    anomalies = 0

    print(f"\n--- Analizando {total} imágenes en '{directory}' ---\n")

    for filename in files:
        path = os.path.join(directory, filename)
        img = load_image(path)
        flag, error = is_anomaly(img, autoencoder, threshold)

        print(f"{filename}: {'ANOMALÍA' if flag else 'NORMAL'} | Error: {error:.6f}")
        if flag:
            anomalies += 1

    print("\n--- Resumen ---")
    print(f"Total: {total}")
    print(f"Anomalías detectadas: {anomalies}")
    print(f"Porcentaje de anomalías: {anomalies / total * 100:.2f}%")

directory_path = "Backend/tensor/dataset/cropped_images"

check_directory(directory_path, autoencoder, threshold)
