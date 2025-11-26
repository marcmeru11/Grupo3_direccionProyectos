import os
import json

class JSONtoYOLO:
    """
    A utility class for converting LabelMe JSON annotation files 
    into YOLO-compatible text files for object detection training.

    The class assumes that each JSON file follows the LabelMe format, including "shapes", "imageWidth", and "imageHeight" keys.
    """

    def __init__(self, data_dir="Backend/YOLO/Data/cropped_images"):
        """
        Initialize the converter.
        """
        self.data_dir = data_dir

    def _convert_shape_to_yolo(self, shape, img_w, img_h):
        """
        Convert a LabelMe shape to YOLO format.

        YOLO format: 
            class_id, x_center, y_center, width, height
            - All coordinates are normalized

        Args:
            shape (dict): A shape dictionary from LabelMe JSON containing "points".

        Returns:
            tuple: (x_center, y_center, width, height) normalized values.
        """
        points = shape["points"]
        xs = [p[0] for p in points]
        ys = [p[1] for p in points]
        x_min, x_max = min(xs), max(xs)
        y_min, y_max = min(ys), max(ys)

        # Compute normalized YOLO coordinates
        x_center = ((x_min + x_max) / 2) / img_w
        y_center = ((y_min + y_max) / 2) / img_h
        width = (x_max - x_min) / img_w
        height = (y_max - y_min) / img_h

        return x_center, y_center, width, height
    
    def _create_file(self, txt_path, txt_lines):
        """
        Write YOLO label data to a text file.

        """
        with open(txt_path, "w") as f:
            f.write("\n".join(txt_lines))

    def generate(self):
        """
        Main method to generate YOLO annotation files.

        Iterates through the directory, converting JSON annotations to YOLO format.
        - If a corresponding JSON file exists for an image, it extracts bounding boxes and writes them to a .txt file.
        - If no JSON exists for an image, assigns it to the default class '0' with a bounding box covering the whole image.
        """
        for file in os.listdir(self.data_dir):
            # Process all JSON files
            if file.endswith(".json"):
                json_path = os.path.join(self.data_dir, file)
                with open(json_path, "r") as f:
                    data = json.load(f)

                img_w = data["imageWidth"]
                img_h = data["imageHeight"]
                txt_lines = []

                # Convert each shape annotation to YOLO format
                for shape in data["shapes"]:
                    label = shape["label"]
                    x_center, y_center, width, height = self._convert_shape_to_yolo(shape, img_w, img_h)
                    
                    # TODO: replace '1' with a proper class ID lookup based on `label`
                    txt_lines.append(f"1 {x_center:.6f} {y_center:.6f} {width:.6f} {height:.6f}")

                # Create a YOLO .txt file corresponding to the JSON file
                txt_path = os.path.join(self.data_dir, file.replace(".json", ".txt"))
                self._create_file(txt_path, txt_lines)
                continue

            # Handle images with no annotation JSON (assumed background class)
            elif file.endswith(".png") and not os.path.exists(os.path.join(self.data_dir, file.replace(".png", ".json"))):
                txt_lines = ["0 0.5 0.5 1.0 1.0"]  # Default full-image bounding box for class 0
                txt_path = os.path.join(self.data_dir, file.replace(".png", ".txt"))
                self._create_file(txt_path, txt_lines)
