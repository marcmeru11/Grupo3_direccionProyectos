from fastapi import FastAPI
from fastapi.responses import JSONResponse
from pydantic import BaseModel
import base64
from io import BytesIO
from PIL import Image
from tensor.src.tensor_validate import evaluate_image
import uvicorn
import os
import tempfile

app = FastAPI()

class ImageRequest(BaseModel):
    model: str
    image_base64: str

def guardar_imagen(bytes_imagen):
    temp = tempfile.NamedTemporaryFile(delete=False, suffix=".png", dir = "Backend/tmp")
    temp.write(bytes_imagen)
    temp.close()
    return temp.name

@app.post("/procesar")
async def procesar_imagen(data: ImageRequest):

    image_bytes = base64.b64decode(data.image_base64)
    image = Image.open(BytesIO(image_bytes))

    if data.model == "YOLO":
        temp_path = guardar_imagen(image_bytes)
        response = evaluate_image(temp_path)
        os.remove(temp_path)

    elif data.model == "tensorflow":
        response = evaluate_image(image)
    else:
        return JSONResponse({"error": "Model not recognized"}, status_code=400)

    return JSONResponse(response)

if __name__ == "__main__":
    uvicorn.run(
        "main:app",
        host="0.0.0.0",
        port=8000,
        reload=True
    )