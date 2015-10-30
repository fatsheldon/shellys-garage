#include <SoftwareSerial.h>

SoftwareSerial bluetooth(2,3);
bool serialMode = false;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  for(int thisPin = 4; thisPin < 10; thisPin++){
    pinMode(thisPin, OUTPUT);
    digitalWrite(thisPin,HIGH);
  }
  
  bluetooth.begin(115200);
  bluetooth.print("$");
  bluetooth.print("$");
  bluetooth.print("$");
  delay(100);
  bluetooth.println("U,9600,N");
  bluetooth.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
  if(bluetooth.available())
  {
    char input = (char)bluetooth.read();
    Serial.print(input);
    if(!serialMode){
    switch(input){
      case '1':
        digitalWrite(4,LOW);
        delay(1000);
        digitalWrite(4,HIGH);
        break;
      case '2':
        digitalWrite(5,LOW);
        delay(1000);
        digitalWrite(5,HIGH);
        break;
      case '3':
        digitalWrite(6,LOW);
        delay(1000);
        digitalWrite(6,HIGH);
        break;
      case 'q':
        digitalWrite(7,LOW);
        delay(1000);
        digitalWrite(7,HIGH);
        break;
      case 'w':
        digitalWrite(8,LOW);
        delay(1000);
        digitalWrite(8,HIGH);
        break;
      case 'e':
        digitalWrite(9,LOW);
        delay(1000);
        digitalWrite(9,HIGH);
        break;
    }
    }
  }
  if(Serial.available())
  {
    char input = (char)Serial.read();
    if(input == '$')
      serialMode = true;
    if(input == '%')
      serialMode = false;
    
    bluetooth.print(input);
  }
}
