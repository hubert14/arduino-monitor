#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <IRremote.h>

IRrecv irReceiver(4); // 4 - digital port for receive IR signals
LiquidCrystal_I2C lcd(0x27,16,2);

// After change this symbols change it on the server application
const char LCD_CLEAR = '@';
const char LINE_BREAK = '$';

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
    Serial.println(irReceiver.decodedIRData.decodedRawData, HEX);
    irReceiver.resume();
  }
  
  while (Serial.available() > 0) {
    char a = Serial.read();
    if(a == LCD_CLEAR) {
      lcd.clear();
      lineCounter = 0;
      charCounter = 0;
    }
    else if(a == LINE_BREAK) {
      lineCounter++;
      charCounter = 0;
    }
    else lcd.setCursor(charCounter++, lineCounter); lcd.print(a);
  }  
}
