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
String message, mex1, mex2, mex3;
int x_pos, y_pos, color;

void setup() {

  matrix.begin();
  matrix.setCursor(3, 12);  
  matrix.setTextColor(matrix.Color333(0,7,3));
  matrix.println("Pronta!");
  delay(3000);
  matrix.fillScreen(matrix.Color333(0,0,0));
  Serial.begin(9600);
}

/*all'interno del loop avverrano le seguenti cose:
 * LETTURA SERIALE STRINGA
 * le stringhe sono formattate in questo modo:
 *  "x_pos:y_pos:color"
 *  x_pos = coordinata x  (int)
 *  y_pos = coordinata y  (int)
 *  color = stringa che conterra' il nome del colore da usare per il led (string) 
 * INIZIO FLOW DI DISEGNO 
 * if color == "black" then cancel
 * else draw
 * DRAW : accende un pixel per volta del colore corretto (con delay per effetto di accensione)
 * CANCEL : spegne tutto un percorso (spegnimento del percorso a segmenti, immediatamente, senza delay
 *          la stringa colore sara' "black"
 * REITERAZIONE DEL LOOP
 * 
 * TODO : verificare la validita' del blinking di due percorsi sovrapposti (metodo difficile)
 *        colorazione di un colore differente per le parti intersecate (metodo facile)
 *        ce ne sbattiamo le palle (metodo fancazzista)
*/
void loop() {
 
 if(Serial.available() > 0) {
    //mex1 = Serial.readStringUntil(':');
    //Serial.read();
    //mex2 = Serial.readStringUntil(':');
    //Serial.read();
    //mex3 = Serial.readStringUntil('.');
    //x_pos = mex1.toInt();
    //y_pos = mex2.toInt();
    //color = mex3.toInt();
    x_pos = Serial.parseInt();
    y_pos = Serial.parseInt();
    color = Serial.parseInt();
    
    switch(color) {

      case 1: //red
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(7, 0, 0));
        delay(500);
        break;

      case 2: //green
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 7, 0));
        //delay(500);
        break;

      case 3: //blue
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 0, 7));
        //delay(200);
        break;

      case 4: //yellow
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 0, 7));
        delay(500);
        
      case 5: //wall
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(3, 7, 3));
        break;

      case 6: //shelf
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(3, 0, 7));
        break;

      case -1: //black
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 0, 0));
        break;

      default:
        break;
      }
  }     
}//fine del loop
