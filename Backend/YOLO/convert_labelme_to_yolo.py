import os
import json

# Directorio donde están las imágenes y los JSON
DATA_DIR = "Data/cropped_images"

# Clase única: "defecto"
CLASS_MAP = {"rafaga": 0}

def convert_shape_to_yolo(shape, img_w, img_h):
    points = shape["points"]
    xs = [p[0] for p in points]
    ys = [p[1] for p in points]
    x_min, x_max = min(xs), max(xs)
    y_min, y_max = min(ys), max(ys)

    x_center = ((x_min + x_max) / 2) / img_w
    y_center = ((y_min + y_max) / 2) / img_h
    width = (x_max - x_min) / img_w
    height = (y_max - y_min) / img_h

    return x_center, y_center, width, height

def main():
    for file in os.listdir(DATA_DIR):
        if file.endswith(".json"):
            json_path = os.path.join(DATA_DIR, file)
            with open(json_path, "r") as f:
                data = json.load(f)
            
            img_w = data["imageWidth"]
            img_h = data["imageHeight"]
            txt_lines = []

            for shape in data["shapes"]:
                label = shape["label"]
                if label not in CLASS_MAP:
                    continue
                cls_id = CLASS_MAP[label]
                x_center, y_center, width, height = convert_shape_to_yolo(shape, img_w, img_h)
                txt_lines.append(f"{cls_id} {x_center:.6f} {y_center:.6f} {width:.6f} {height:.6f}")

            # Crear el archivo .txt correspondiente
            txt_path = os.path.join(DATA_DIR, file.replace(".json", ".txt"))
            with open(txt_path, "w") as f:
                f.write("\n".join(txt_lines))

if __name__ == "__main__":
    main()
