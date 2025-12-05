FROM python:3.9-slim

WORKDIR /app
ENV PYTHONPATH=/app/Backend

# Instalar dependencias del sistema (necesarias para TensorFlow y PIL)
RUN apt-get update && apt-get install -y \
    libglib2.0-0 \
    libsm6 \
    libxrender1 \
    libxext6 \
    libgl1 \
    && rm -rf /var/lib/apt/lists/*

# Instalar dependencias de Python
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copiar todo el código
COPY . .

# Exponer el puerto
EXPOSE 8000

# Ejecutar FastAPI
CMD ["uvicorn", "Backend.main:app", "--host", "0.0.0.0", "--port", "8000"]
