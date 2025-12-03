from ultralytics import YOLO
import cv2
import base64

class YOLOMdoel:
    
    model = YOLO("/app/Backend/YOLO/best.pt")
    last_prediction = None

    @staticmethod
    def img_to_base64(image_array):
        success, buffer = cv2.imencode(".png", image_array)
        if not success:
            raise ValueError("No se pudo convertir la imagen a PNG")
        return base64.b64encode(buffer).decode("utf-8")

    @staticmethod
    def predict(image_path, conf_threshold=0.5, save=False, project="runs/detect", name="predict"):
        if YOLOMdoel.model is None:
            print("El modelo no está cargado.")
            return None

        try:
            results = YOLOMdoel.model(
                image_path,
                conf=conf_threshold,
                save=save,
                project=project,
                name=name
            )

            YOLOMdoel.last_prediction = results
            annotated_frame = results[0].plot()
            annotated_base64 = YOLOMdoel.img_to_base64(annotated_frame)

            status = "NORMAL" if int(results[0].boxes.cls[0]) == 0 else "ANOMALY"

            return {
                "reconstructed_base": annotated_base64,
                "status": status
            }

        except Exception as e:
            print(f"Error during prediction: {e}")
            return None

    @staticmethod
    def evaluate_image(image_path):
        return YOLOMdoel.predict(image_path)
