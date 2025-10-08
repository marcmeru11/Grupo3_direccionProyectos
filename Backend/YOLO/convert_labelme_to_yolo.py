import os
import json

#data directory
DATA_DIR = "Data/cropped_images"

#extract and parse to yolo format from labelme json
#yolo format: <object-class> <x_center> <y_center> <width> <height>
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
    for file in os.listdir(DATA_DIR): #take all the files in the data directory
        if file.endswith(".json"): #take the json files only
            json_path = os.path.join(DATA_DIR, file)
            with open(json_path, "r") as f:
                data = json.load(f)
            
            img_w = data["imageWidth"] #navigation of the json structure
            img_h = data["imageHeight"]
            txt_lines = []

            for shape in data["shapes"]:
                label = shape["label"]
                
                
                
                x_center, y_center, width, height = convert_shape_to_yolo(shape, img_w, img_h)
                txt_lines.append(f"1 {x_center:.6f} {y_center:.6f} {width:.6f} {height:.6f}")

            # Creates a txt file for each json file in yolo format
            txt_path = os.path.join(DATA_DIR, file.replace(".json", ".txt"))
            with open(txt_path, "w") as f:
                f.write("\n ".join(txt_lines))
            continue

        elif file.endswith(".png") and not os.path.exists(os.path.join(DATA_DIR, file.replace(".png", ".json"))): #detects if the data belongs to the 0 class 
        
           
            
            txt_path = os.path.join(DATA_DIR, file.replace(".png", ".txt"))
            with open(txt_path, "w") as f:
                f.write("0 0.5 0.5 1.0 1.0")  
            continue

if __name__ == "__main__":
    main()
