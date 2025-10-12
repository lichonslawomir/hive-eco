
#include <driver/i2s.h>

#define SAMPLE_BITS 32

#define I2S_WS   15  // D15
#define I2S_SD   32  // D32
#define I2S_SCK  14  // D14

#define I2S_PORT I2S_NUM_0

esp_err_t i2s_install()
{
  const i2s_config_t i2s_config = {
      .mode = i2s_mode_t(I2S_MODE_MASTER | I2S_MODE_RX),
      .sample_rate = 44100,
      .bits_per_sample = i2s_bits_per_sample_t(SAMPLE_BITS),
      .channel_format = I2S_CHANNEL_FMT_ONLY_RIGHT,
      .communication_format = I2S_COMM_FORMAT_STAND_I2S,
      .intr_alloc_flags = ESP_INTR_FLAG_LEVEL1, // default interrupt priority
      .dma_buf_count = 4,
      .dma_buf_len = 1024,
      //.use_apll = true
      .use_apll = false,
      .tx_desc_auto_clear = false,
      .fixed_mclk = 0
    };

  return i2s_driver_install(I2S_PORT, &i2s_config, 0, NULL);
}

esp_err_t i2s_setpin()
{
  /*i2s_pin_config_t pin_config = {
    .bck_io_num = I2S_SCK,
    .ws_io_num = I2S_WS,
    .data_out_num = -1, // Not used
    .data_in_num = I2S_SD
  };*/

  const i2s_pin_config_t pin_config = {
      .bck_io_num = I2S_SCK,
      .ws_io_num = I2S_WS,
      .data_out_num = -1,
      .data_in_num = I2S_SD};

  return i2s_set_pin(I2S_PORT, &pin_config);
}