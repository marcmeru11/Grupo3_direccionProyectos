FROM python:3.9-slim

WORKDIR /app

ENV PYTHONPATH=/app/Backend

# Instalar dependencias del sistema (tensorflow lo necesita)
RUN apt-get update && apt-get install -y \
    libglib2.0-0 \
    libsm6 \
    libxrender1 \
    libxext6 \
    && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

# FastAPI + Uvicorn en el puerto 8000
EXPOSE 8000
CMD ["uvicorn", "Backend.main:app", "--host", "0.0.0.0", "--port", "8000"]