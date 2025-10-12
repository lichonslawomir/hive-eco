#include <Arduino.h>
#include <driver/i2s.h>
#include <DHT.h>
#include "base64.h"

#define LED_PIN 2

#define DHTPIN1  4         //SDA
#define DHTPIN2  5         //SDB
#define DHTTYPE DHT22

#define I2S_WS        15
#define I2S_SD        32
#define I2S_SCK       14
//GND - BLACK
//VDD - BLUE

#define SERIAL_BAUD   921600
#define BUFFER_SIZE   1024 // bytes (number of samples per chunk)

void setupI2S() {
  i2s_config_t i2s_config = {
    .mode = (i2s_mode_t)(I2S_MODE_MASTER | I2S_MODE_RX),
    .sample_rate = 16000,
    .bits_per_sample = I2S_BITS_PER_SAMPLE_16BIT,
    .channel_format = I2S_CHANNEL_FMT_ONLY_LEFT,
    .communication_format = I2S_COMM_FORMAT_STAND_I2S,//I2S_COMM_FORMAT_I2S_MSB,
    .dma_buf_count = 4,
    .dma_buf_len = 1024,
    .use_apll = false,
    .tx_desc_auto_clear = false,
    .fixed_mclk = 0
  };

  i2s_pin_config_t pin_config = {
    .bck_io_num = I2S_SCK,
    .ws_io_num = I2S_WS,
    .data_out_num = -1,      // Not used
    .data_in_num = I2S_SD
  };

  i2s_driver_install(I2S_NUM_0, &i2s_config, 0, NULL);
  i2s_set_pin(I2S_NUM_0, &pin_config);
}

// ---- DHT22 Setup ----
DHT dht1(DHTPIN1, DHTTYPE);
DHT dht2(DHTPIN2, DHTTYPE);
unsigned long lastSendMillis = 0;
//const unsigned long sendIntervalMillis = 1000 * 60 * 5;
const unsigned long sendIntervalMillis = 1000 * 10;

static uint8_t buffer[BUFFER_SIZE];
size_t bytes_read = 0;
char cmd[6];
const char hiveId = '2';

void setup() {
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, HIGH);

  Serial.begin(SERIAL_BAUD);
  setupI2S();
  dht1.begin();
  dht2.begin();

  digitalWrite(LED_PIN, LOW);
}

void loop() {
  i2s_read(I2S_NUM_0, buffer, BUFFER_SIZE, &bytes_read, portMAX_DELAY);

  sprintf(cmd, "A%c %d ", hiveId, bytes_read);
  Serial.print(cmd);
  Serial.write(buffer, bytes_read);
  Serial.print("C");

  if (millis() - lastSendMillis >= sendIntervalMillis) {
    float h1 = dht1.readHumidity();
    float t1 = dht1.readTemperature();

    float h2 = dht2.readHumidity();
    float t2 = dht2.readTemperature();

    Serial.print("H");
    Serial.write((uint8_t *)&h1, sizeof(float));
    
    Serial.print("T");
    Serial.write((uint8_t *)&t1, sizeof(float));
    
    Serial.print("G");
    Serial.write((uint8_t *)&h2, sizeof(float));

    Serial.print("U");
    Serial.write((uint8_t *)&t2, sizeof(float));
    
    Serial.print("E");

    lastSendMillis = millis();
  }
}