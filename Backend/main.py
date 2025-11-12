from fastapi import FastAPI, Request
from fastapi.responses import JSONResponse
from pydantic import BaseModel
import base64
from io import BytesIO
from PIL import Image, ImageOps
import tensor_validate as tf

app = FastAPI()

class ImageRequest(BaseModel):
    model: str
    image_base64: str

@app.post("/procesar")
async def procesar_imagen(data: ImageRequest):

    image_bytes = base64.b64decode(data.image_base64)
    image = Image.open(BytesIO(image_bytes))

    if data.model == "YOLO":
        # yolo here
    elif data.model == "tensorflow":
        response = tf.evaluate_image(image)
    else:
        return JSONResponse({"error": "Model not recognized"}, status_code=400)

    return JSONResponse(response)