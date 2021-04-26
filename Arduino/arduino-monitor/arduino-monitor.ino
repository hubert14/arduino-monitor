#include <Wire.h>                 // Подключаем библиотеку Wire
#include <LiquidCrystal_I2C.h>    // Подключаем библиотеку LiquidCrystal_I2C
#include <IRremote.h>

IRrecv irReceiver(4); // указываем вывод, к которому подключён приёмник
LiquidCrystal_I2C lcd(0x27,16,2); // Задаем адрес и размерность дисплея.

const char LCD_CLEAR = '@';
const char LINE_BREAK = '$';

void setup()
{
  Serial.begin(9600); 
  irReceiver.enableIRIn(); // запускаем приём
  lcd.init();                     // Инициализация lcd дисплея
  lcd.backlight();                // Включение подсветки дисплея
}

void loop() {
  // put your main code here, to run repeatedly:
  int charCounter = 0;
  int lineCounter = 0;

  if (irReceiver.decode()) { // если данные пришли
    Serial.println(irReceiver.decodedIRData.decodedRawData, HEX); // выводим данные
    irReceiver.resume(); // принимаем следующую команду
  }
   
  while (Serial.available() > 0) {
    char a = Serial.read();
    if(a == LCD_CLEAR) {
      lcd.clear();
      lineCounter = 0;
      charCounter = 0;
    }
    else if(a == LINE_BREAK) 
    {
      lineCounter++;
      charCounter = 0;
    }    
    else 
    {
      lcd.setCursor(charCounter++, lineCounter); lcd.print(a); 
    }
  }  
}
