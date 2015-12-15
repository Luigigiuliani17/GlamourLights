// testshapes demo for RGBmatrixPanel library.
// Demonstrates the drawing abilities of the RGBmatrixPanel library.
// For 32x64 RGB LED matrix.

// NOTE THIS CAN ONLY BE USED ON A MEGA! NOT ENOUGH RAM ON UNO!

#include <Adafruit_GFX.h>   // Core graphics library
#include <RGBmatrixPanel.h> // Hardware-specific library

#define OE   9
#define LAT 10
#define CLK 11
#define A   A0
#define B   A1
#define C   A2
#define D   A3

RGBmatrixPanel matrix(A, B, C, D, CLK, LAT, OE, false, 64);
int message;

void setup() {

  matrix.begin();
  Serial.begin(9600);
  
  // draw a pixel in solid white
  matrix.drawPixel(10, 10, matrix.ColorHSV(-1200, 250, 100, true));
  matrix.drawPixel(13, 13, matrix.ColorHSV(240, 60, 78, true)); 
  matrix.drawPixel(13, 16, matrix.ColorHSV(7, 60, 78, true));
  matrix.drawPixel(10, 13, matrix.Color333(7,7,0));
  
delay(1000);
   matrix.fillScreen(matrix.Color333(0,0,0));
}

void loop() {
if (Serial.available() > 0) { // Check to see if there is a new message
message = Serial.read(); // Put the serial input into the message

switch(message){

 case 'a':
 // If a capitol A is received
 for(int i=10; i<20; i++) {    
  
    matrix.drawPixel(2,i, matrix.Color333(0,2,7));
    delay(100);
 }
   for(int i=2; i<12; i++) {

    matrix.drawPixel(i,19, matrix.Color333(0,2,7));
    delay(100);
    }
   
   for(int i=19; i>9; i--) {

    matrix.drawPixel(12,i, matrix.Color333(0,2,7));
    matrix.drawPixel(30,i, matrix.Color333(7,2,7));
    delay(100);
    }

   for(int i=12; i>1; i--) {

    matrix.drawPixel(i,10, matrix.Color333(0,2,7));
    delay(100);
    }

     for(int i=19; i>9; i--) {

    matrix.drawPixel(30,i, matrix.Color333(0,0,0));
    delay(100);
    }
    break;
 
 case 'b':
 matrix.fillScreen(matrix.Color333(0,0,0));
 break;

 case 'c':
 matrix.drawRect(0, 0, matrix.width(), matrix.height(), matrix.Color333(7, 7, 0));
 break;

case 'd':
matrix.setCursor(3, 12);  
 matrix.setTextColor(matrix.Color333(7,1,1));
 delay(3000);
  matrix.println("Dario Culo");
break;
}
}
}


// Input a value 0 to 24 to get a color value.
// The colours are a transition r - g - b - back to r.
uint16_t Wheel(byte WheelPos) {
  if(WheelPos < 8) {
   return matrix.Color333(7 - WheelPos, WheelPos, 0);
  } else if(WheelPos < 16) {
   WheelPos -= 8;
   return matrix.Color333(0, 7-WheelPos, WheelPos);
  } else {
   WheelPos -= 16;
   return matrix.Color333(0, WheelPos, 7 - WheelPos);
  }
}
