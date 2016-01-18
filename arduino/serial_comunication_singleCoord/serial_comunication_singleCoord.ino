#include <Adafruit_GFX.h>   // Core graphics library
#include <RGBmatrixPanel.h> // Hardware-specific library

#define OE   9
#define LAT 10
#define CLK 11
#define A   A0
#define B   A1
#define C   A2
#define D   A3
#define redLED1 51
#define redLED2 39
#define greenLED1 53
#define blueLED1 49
#define blueLED2 41

RGBmatrixPanel matrix(A, B, C, D, CLK, LAT, OE, false, 64);
String message, mex1, mex2, mex3;
int x_pos, y_pos, color;

void setup() {

  pinMode(redLED1, OUTPUT);
  pinMode(redLED2, OUTPUT);
  pinMode(greenLED1, OUTPUT);
  pinMode(blueLED1, OUTPUT);
  pinMode(blueLED2, OUTPUT);
  
  matrix.begin();
  matrix.setCursor(3, 12);  
  matrix.setTextColor(matrix.Color333(0,7,3));
  matrix.println("Pronta!");
  delay(3000);
  matrix.fillScreen(matrix.Color333(0,0,0));
  Serial.begin(38400);
}

/* inside loop these things will happen :
 * SERIAL READING OF THE STRING
 * strings are formatted in this way, for the carpet led:
 *  "x_pos:y_pos:color."
 *  x_pos = x coord  (int)
 *  y_pos = y coord (int)
 *  color = color code for led switching on (int) 
 * and for bigger LEDlights will be this:
 * "-1:-1:LEDid" = to switch on the specific led
 * "-2:-2:LEDid" = to switch off the specific led 
 * LIGHTS SWITCHING ON OR OFF 
 * CARPET DRAW BEGINNING 
 * if color == "black" then cancel
 * else draw
 * DRAW : switch on the right led of the right color for the path
 * CANCEL : switch off all the path
 * LOOP REITERATION 
*/
void loop() {

 if(Serial.available()>3) {

    //Here string is splitted in the correct way
    message = Serial.readStringUntil('.');
    
    int commaIndex = message.indexOf(':');
    int secondCommaIndex = message.indexOf(':', commaIndex+1);

    x_pos = message.substring(0, commaIndex).toInt();
    y_pos = message.substring(commaIndex+1, secondCommaIndex).toInt();
    color = message.substring(secondCommaIndex+1, message.length()).toInt();

  //Code to switch on the lights
  if(x_pos == -1) {

    int code = color;
    switch(code) {

      case 1:
        digitalWrite(redLED1, HIGH);
        break;

      case 2:
        digitalWrite(redLED2, HIGH);
        break;

       case 3:
        digitalWrite(greenLED1, HIGH);
        break;

       case 4:
        digitalWrite(blueLED1, HIGH);
        break;

       case 5:
        digitalWrite(blueLED2, HIGH);
        break;
    }
  }

  //Switch lights off
  if(x_pos == -2) {

    int code = color;
    switch(code) {

      case 1:
        digitalWrite(redLED1, LOW);
        break;

      case 2:
        digitalWrite(redLED2, LOW);
        break;

       case 3:
        digitalWrite(greenLED1, LOW);
        break;

       case 4:
        digitalWrite(blueLED1, LOW);
        break;

       case 5:
        digitalWrite(blueLED2, LOW);
        break;
    }
  }

  //Here the switch with the color code in which the leds will be switched on (or off)
  if(x_pos >= 0) {
    switch(color) {

      case 1: //red
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(7, 0, 0));
        break;

      case 2: //green
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 7, 0));
        break;

      case 3: //blue
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 0, 7));
        break;

      case 4: //yellow
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(7, 7, 0));
        break;
        
      case 5: //wall light white
        matrix.drawPixel(x_pos, y_pos, matrix.Color888(100, 100, 100, true));
        break;

      case 6: //shelf pink
        matrix.drawPixel(x_pos, y_pos, matrix.Color888(153, 0, 76, true));
        break;

      case 7://hotspot white
        matrix.drawPixel(x_pos, y_pos, matrix.Color888(150, 150, 150, false));
        break;

      case -1: //black
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 0, 0));
        break;

      default:
        break;
      }
    } 
 }    
}//end of loop
