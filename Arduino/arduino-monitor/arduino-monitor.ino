#include <Wire.h>
#include <LiquidCrystal_PCF8574.h>
#include <IRremote.h>
IRrecv irReceiver(4); // 4 - digital port for receive IR signals
LiquidCrystal_PCF8574 lcd(0x27);

// SYMBOLS
const char LCD_POWER_SYMBOL = '!';
const char LCD_POWER_CHECK_SYMBOL = '^';
const char LCD_CLEAR_SYMBOL = '@';
const char LINE_BREAK_SYMBOL = '$';

// HEADERS
const String IR_COMMAND_HEADER = "IR_COMMAND";
const String SCREEN_ON_STATUS_HEADER = "SCREEN_ON_STATUS";

// IR
const String IR_EQ_COMMAND = "F609FF00";

bool _isScreenOn = true;

void setup()
{
  Wire.begin();
  Wire.beginTransmission(0x27);
  irReceiver.enableIRIn();
  lcd.begin(16, 2);
  lcd.setBacklight(1);
  Serial.begin(9600); 
}

void loop() {
  checkIrReceiver();
  readFromSerial();
}

void checkIrReceiver() {
  if (irReceiver.decode()) {
    String decoded = String(irReceiver.decodedIRData.decodedRawData, HEX);
    decoded.toUpperCase();
    
    if(decoded == IR_EQ_COMMAND) 
      changeLcdPower();
    
    sendToSerial(IR_COMMAND_HEADER, decoded);
    irReceiver.resume();
  }
}

void readFromSerial() {
  int charCounter = 0;
  int lineCounter = 0;
  
  while (Serial.available() > 0) {
    char symbol = Serial.read();
    switch(symbol){
      case LCD_POWER_CHECK_SYMBOL:
          sendToSerial(SCREEN_ON_STATUS_HEADER, String(_isScreenOn));
          break;
      case LCD_POWER_SYMBOL:
          changeLcdPower();
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
          lcd.setCursor(charCounter++, lineCounter); 
          lcd.print(symbol);
          break;
    }
  }
}

void changeLcdPower() {
  _isScreenOn = !_isScreenOn;
  if(_isScreenOn) {
      lcd.display();
      lcd.setBacklight(1);
  } else {
      lcd.noDisplay();
      lcd.setBacklight(0);
  }
  sendToSerial(SCREEN_ON_STATUS_HEADER,String(_isScreenOn));
}

void sendToSerial(String header, String command) {
  Serial.println(header + ":" + command);
}
