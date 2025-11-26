import cv2
from ultralytics import YOLO
import base64
import numpy

"""
This module defines the `YOLOMdoel` class — a lightweight interface for loading a YOLO model
(using the `ultralytics` library) and performing object detection on single or multiple images.

The class provides:
- Model loading from a `.pt` YOLO weights file.
- Single or batch image prediction.
- Visualization of annotated results.
- Configurable confidence thresholds and optional result saving.
"""


"""
    A class for handling a YOLO object detection model using the Ultralytics API.

    It allows you to perform inference on single or multiple images, visualize results,
    and keep track of the last prediction made.

    Attributes:
   
        model : YOLO
            The loaded YOLO model instance.
        last_prediction : ultralytics.engine.results.Results | None
            The most recent prediction made. Updated after each call to `predict` or `predict_multiple`.
"""
class YOLOMdoel:
    
    """
        Initializes the YOLO model from the given file path.

        Parameters:
        
            model_path : str
                Path to the `.pt` YOLO model file.
    """
    def __init__(self, model_path: str) -> None:
        try:
            self.last_prediction = None
            self.model = YOLO(model_path)
            print(f"model taken from {model_path}")

        except Exception as e:
            print(f"Error: {e}")
            self.model = None

    """
        Performs object detection on a single image using the loaded YOLO model.

        Parameters:
       
            image_path : str
                Path to the image file for detection.
            conf_threshold : float, optional
                Minimum confidence threshold for displaying detections (default is 0.5).
            save : bool, optional
                Whether to save the annotated image to disk (default is False).
            project : str, optional
                Directory where results will be stored (default is "runs/detect").
            name : str, optional
                Subdirectory name for this specific run (default is "predict").
    """
    def evaluate_image(image_path):
        predict(self, image_path)

    def img_to_base64(image_array):
        success, buffer = cv2.imencode(".png", img_array)
        if not success:
            raise ValueError("No se pudo convertir la imagen a PNG")

        img_base64 = base64.b64encode(buffer).decode("utf-8")
        return img_base64
    
    def predict(self, image_path, conf_threshold=0.5, save=False, project="runs/detect", name="predict"):
        if self.model is None:
            print("El modelo no está cargado.")
            return None

        try:
            results = self.model(image_path,
                                 conf=conf_threshold, 
                                 save=save, 
                                 project=project, 
                                 name=name)
            
            self.last_prediction = results
            annotated_frame = results[0].plot()
            annotated_base64 = img_to_base64(annotated_frame)
            #TODO pasar imagen (annotated_frame) a base64
            if int(results[0].boxes.cls[0]) == 0:
                status = "NORMAL"
            else:
                status = "ANOMALY"

            result = {
                "reconstructed_base": annotated_base64, #TODO annotated_base64 poner,
                "status": status
            }
            
            return result
        except Exception as e:
            print(f"Error during prediction: {e}")
            return None
        
        
    """
        Displays the most recent prediction in an OpenCV window.

        The window will remain open until a key is pressed.
        Requires that `predict()` or `predict_multiple()` has been called first.
    """
    
"""
    Main execution block.

    Loads the YOLO model (`best.pt`), performs a prediction on a sample image,
    and displays the annotated result.
"""
if __name__ == "__main__":
    yolo = YOLOMdoel("best.pt")