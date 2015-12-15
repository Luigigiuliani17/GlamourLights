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
String mex1;
String mex2,mex3,mex4;


void setup() {

  matrix.begin();
  Serial.begin(9600);
  
  // draw a pixel in solid white
  matrix.setCursor(3, 12);  
 matrix.setTextColor(matrix.Color333(3,7,1));
  matrix.println("AUI MATRIX!");
  
delay(2000);
   matrix.fillScreen(matrix.Color333(0,0,0));
}

void loop() {
if (Serial.available() > 0) { // Check to see if there is a new message
mex1 = Serial.readStringUntil(':'); // Put the serial input into the message
Serial.read();
mex2 = Serial.readStringUntil(':');
Serial.read();
mex3 = Serial.readStringUntil(':');
Serial.read();
mex4 = Serial.readString();
int option = mex1.toInt();
switch(option){

 case 1:
 {
 // draw a square of lenght mex4 and starting from pos mex2, mex3
   int x = mex2.toInt();
   int y = mex3.toInt();
   int lenght = mex4.toInt();
   for(int i=x; i<x+lenght; i++) {    
      matrix.drawPixel(y,i, matrix.Color333(0,2,7));
      delay(100);
   }
 
   for(int i=y; i<y+lenght; i++) {
    matrix.drawPixel(i,x+lenght, matrix.Color333(0,2,7));
    delay(100);
    }
   
   for(int i=x+lenght; i>x-1; i--) {
    matrix.drawPixel(y+lenght,i, matrix.Color333(0,2,7));
    matrix.drawPixel(30,i, matrix.Color333(7,2,7));
    delay(100);
    }

   for(int i=y+lenght; i>y-1; i--) {
    matrix.drawPixel(i,x, matrix.Color333(0,2,7));
    delay(100);
    }

   for(int i=x+lenght; i>x-1; i--) {
    matrix.drawPixel(30,i, matrix.Color333(0,0,0));
    delay(100);
    }

 break;
}
case 2:
  if(mex2.toInt()== 2){
     matrix.fillScreen(matrix.Color333(0,0,0));
    }
   break;

 case 3:
     matrix.drawRect(0, 0, matrix.width(), matrix.height(), matrix.Color333(7, 7, 0));
     break;

case 4:
  matrix.setCursor(3, 12);  
   matrix.setTextColor(matrix.Color333(7,1,1));
   delay(3000);
    matrix.println("AUI MATRIX!");
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
