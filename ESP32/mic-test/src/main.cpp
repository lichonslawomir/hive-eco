#include "driver/i2s.h"
#include "soc/i2s_reg.h"
#include <driver/i2s.h>
#include <esp32-hal-gpio.h>
#include <HardwareSerial.h>

#define BUFLEN 256

static const i2s_port_t i2s_num = I2S_NUM_0; // i2s port number

static const i2s_config_t i2s_config = {
     .mode = (i2s_mode_t)(I2S_MODE_MASTER | I2S_MODE_RX),
     .sample_rate = 16000,
     .bits_per_sample = I2S_BITS_PER_SAMPLE_32BIT,
     .channel_format = I2S_CHANNEL_FMT_RIGHT_LEFT,
     .communication_format = (i2s_comm_format_t)(I2S_COMM_FORMAT_I2S | I2S_COMM_FORMAT_I2S_MSB),
     .intr_alloc_flags = 0, // default interrupt priority
     .dma_buf_count = 8,
     .dma_buf_len = 64,
     .use_apll = false
};

static const i2s_pin_config_t pin_config = {
    .bck_io_num = 26,
    .ws_io_num = 25,
    .data_out_num = I2S_PIN_NO_CHANGE,
    .data_in_num = 22
};

void setup() {
   pinMode(22, INPUT);
   Serial.begin(921600);
   i2s_driver_install(i2s_num, &i2s_config, 0, NULL);   //install and start i2s driver
   REG_SET_BIT(  I2S_TIMING_REG(i2s_num),BIT(9));   /*  #include "soc/i2s_reg.h"   I2S_NUM -> 0 or 1*/
   REG_SET_BIT( I2S_CONF_REG(i2s_num), I2S_RX_MSB_SHIFT);
   i2s_set_pin(i2s_num, &pin_config);
   Serial.print("H1");
}

int32_t audio_buf[BUFLEN];
char cmd[6];
const char hiveId = '1';

unsigned long lastSendMillis = 0;
const unsigned long sendIntervalMillis = 1000 * 10;
void loop() {
  //size_t bytes_read = 0;
  //i2s_read(i2s_num, (void **)audio_buf, sizeof(int32_t) * 256, &bytes_read, portMAX_DELAY);
  
  //sprintf(cmd, "A%c %d ", hiveId, bytes_read);
  //Serial.print(cmd);
  //Serial.write((uint8_t*)audio_buf, bytes_read);
  //Serial.print("C");

  if (millis() - lastSendMillis >= sendIntervalMillis) {
    Serial.println("H");
    lastSendMillis = millis();
  }
}