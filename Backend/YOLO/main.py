import cv2
from ultralytics import YOLO


class YOLOMdoel:
    def __init__(self, model_path: str) -> None:
        try:
            self.last_prediction = None
            self.model = YOLO(model_path)
            print(f"model taken from {model_path}")

        except Exception as e:
            print(f"Error: {e}")
            self.model = None

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
            
            
            return annotated_frame, int(results[0].boxes.cls[0])
        except Exception as e:
            print(f"Error during prediction: {e}")
            return None
        
    def show_last_prediction(self):

        annotated_frame = self.last_prediction[0].plot() 
        cv2.imshow("YOLO Result", annotated_frame)
        cv2.waitKey(0)
        cv2.destroyAllWindows()
    
    def predict_multiple(self, image_paths, conf_threshold=0.5, save=False, project="runs/detect", name="predict"):
        if self.model is None:
            print("El modelo no está cargado.")
            return None

        predictions = []
        for image_path in image_paths:
            try:
                results = self.model(image_path,
                                     conf=conf_threshold, 
                                     save=save, 
                                     project=project, 
                                     name=name)
                
                
                predictions.append(results)
            except Exception as e:
                print(f"Error during prediction for {image_path}: {e}")
                predictions.append(None)
        
        self.last_prediction = results
        return predictions
    
if __name__ == "__main__":
    yolo = YOLOMdoel("best.pt")
    annotated_frame, result = yolo.predict("Data/cropped_images/img_2025-01-31_10.46.48_11786819_cam_H2674069.png", conf_threshold=0.25, save=False)
    print("the class bellongs to the class: ", result)
    yolo.show_last_prediction()

