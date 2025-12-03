import tensorflow as tf
import numpy as np
import json
from tensor.src import data_loader as dt
import base64


#Configuración general
MODEL_PATH = "/app/Backend/tensor/src/model.keras"
METADATA_PATH = "/app/Backend/tensor/src/model/metadata.json"
OUTPUT_PATH = "/app/Backend/tensor/src/results.json"

IMG_HEIGHT = 500
IMG_WIDTH = 464

#Cargamos el modelo y el threshold calculado
print("Loading model and metadata...")
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

def evaluate_image(img):
    img = tf.convert_to_tensor(np.array(img) / 255.0, dtype=tf.float32)
    img = tf.expand_dims(img, 0)
    reconstructed = model.predict(img, verbose=0)
    error = np.mean(np.abs(img.numpy() - reconstructed))
    is_anomaly = error > threshold

    status = "ANOMALY" if is_anomaly else "NORMAL"

    reconstructed_img = tf.squeeze(reconstructed, axis=0)
    reconstructed_img = tf.image.convert_image_dtype(reconstructed_img, dtype=tf.uint8, saturate=True)
    png_bytes = tf.io.encode_png(reconstructed_img).numpy()
    img_base64 = base64.b64encode(png_bytes).decode("utf-8")

    result = {
        "error": float(error),
        "threshold": float(threshold),
        "status": status.lower(),
        "reconstructed_base64": img_base64
    }
    
    return result