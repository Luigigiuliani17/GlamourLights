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
String mex1, mex2, mex3;
int x_pos, y_pos, color;

void setup() {

  matrix.begin();
  matrix.drawPixel(10,10,matrix.Color333(1,1,1));
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
if (Serial.available() > 0) { 
// Check to see if there is a new message
//i primi 2 sono le posizioni che vengono direttamente trasformate in interi
mex1 = Serial.readStringUntil(':');
Serial.read();
mex2 = Serial.readStringUntil(':'); // Put the serial input into the message
Serial.read();
mex3 = Serial.readString();
y_pos = mex2.toInt();
x_pos = mex1.toInt();
color = mex3.toInt();
//colore e' una stringa la quale verra', della quale verra verificato il valore in if in cascata
//switch che decide l'azione principale (spegnimento/accensione)
//Qui si computano le coordinate colore a seconda delle direttive seriali
//i colori "wall" e "shelf" saranno per i led che identificano scaffali e muri 
//pertanto saranno sempre uguali e accesi sempre

        
    switch(color) {

      case 1:
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(7, 0, 0));
        delay(500);
        break;

      case 2:
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 7, 0));
        delay(500);
        break;

      case 3:
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 0, 7));
        delay(500);
        break;

      case 4:
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(1, 1, 1));
        break;

      case 5:
        matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 1, 1));
        break;

      case 0:
      matrix.drawPixel(x_pos, y_pos, matrix.Color333(0, 0, 0));
        break;
        
      }
  }        
}//fine del loop
