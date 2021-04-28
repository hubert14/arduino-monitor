#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <IRremote.h>

IRrecv irReceiver(4); // 4 - digital port for receive IR signals
LiquidCrystal_I2C lcd(0x27,16,2);

// After change this symbols change it on the server application
const char LCD_POWER_SYMBOL = '!';
const char LCD_POWER_CHECK_SYMBOL = '^';
const char LCD_CLEAR_SYMBOL = '@';
const char LINE_BREAK_SYMBOL = '$';

const String IR_COMMAND_HEADER = "IR_COMMAND";
const String SCREEN_ON_STATUS_HEADER = "SCREEN_ON_STATUS";

boolean isScreenOn = true;

void setup()
{
  Serial.begin(9600); 
  irReceiver.enableIRIn();
  lcd.init();
  lcd.backlight();
}

void loop() {
  int charCounter = 0;
  int lineCounter = 0;

  if (irReceiver.decode()) {
    Serial.print(IR_COMMAND_HEADER + ":");
    Serial.println(irReceiver.decodedIRData.decodedRawData, HEX);
    irReceiver.resume();
  }
  while (Serial.available() > 0) {
    char symbol = Serial.read();
    switch(symbol){
      case LCD_POWER_CHECK_SYMBOL:
          Serial.println(SCREEN_ON_STATUS_HEADER + ":" + isScreenOn);
          break;
      case LCD_POWER_SYMBOL:
          isScreenOn = !isScreenOn;
          if(isScreenOn) lcd.backlight();
          else lcd.noBacklight();
          break;
      case LCD_CLEAR_SYMBOL:
          lcd.clear();
          lineCounter = 0;
          charCounter = 0;
          break;
      case LINE_BREAK_SYMBOL:
          lineCounter++;
          charCounter = 0;
          break;
      default:
          lcd.setCursor(charCounter++, lineCounter); lcd.print(symbol);
          break;
    }
  }
}  
