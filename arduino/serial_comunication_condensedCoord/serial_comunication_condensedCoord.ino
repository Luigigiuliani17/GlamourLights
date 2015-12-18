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
String color;
int x0_pos,x1_pos,y0_pos,y1_pos;
int color_coord[3];

/*qui avvera' l'instanziamento della matrice, con i parametri passatemi dal seriale
 * e in piu' la creazione con colori "tenui" (TODO: check HVS hue)per le aree segnate come
 * non "calpestabili", contenenti uno scaffale a cui arrivare
 * TOASK : segno del punto di inizio del terminale?
 */
void setup() {

  matrix.begin();
  Serial.begin(9600);
}

/*all'interno del loop avverrano le seguenti cose:
 * LETTURA SERIALE STRINGA
 * le stringhe sono formattate in questo modo:
 *  "x0_pos:x1_pos:y0_pos:y1_pos:color"
 *  x0_pos -> x1_pos = coordinata x0 di partenza e coordinata x0 di arrivo (int)
 *  y0_pos -> y1_pos = coordinata y1 di partenza e coordinata y1 di arrivo (int)
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
//i primi quattro sono le posizioni che vengono direttamente trasformate in interi
x0_pos = Serial.readStringUntil(':').toInt(); // Put the serial input into the message
Serial.read();
x1_pos = Serial.readStringUntil(':').toInt();
Serial.read();
y0_pos = Serial.readStringUntil(':').toInt();
Serial.read();
y1_pos = Serial.readStringUntil(':').toInt();
Serial.read();
//colore e' una stringa la quale verra', della quale verra verificato il valore in if in cascata
color = Serial.readStringUntil(':');
Serial.read();

//switch che decide l'azione principale (spegnimento/accensione)

    //Qui si computano le coordinate colore a seconda delle direttive seriali
    if(strcmp(color, "red")
    color_coord = {5,0,0};  
    if(strcmp(color, "green")
    color_coord = {0,5,0};
    if(strcmp(color, "blue")
    color_coord = {0,0,5};
    if(strcmp(color, "yellow")
    color_coord = {0,5,5};
    if(strcmp(color, "black")
    color_coord = {0,0,0};

    //gestione di accensione e spegnimento dei led (lo spegnimento sara' stantaneo a differenza dall'accensione)
    if (x0_pos == x1_pos) { //qui si muove solo la coordinata y
      if(y0_pos < y1_pos) {  //qui ci spostiamo in alto, le y crescono
         int i;
         for (i = y0_pos; i <= y1_pos; i++) {
          matrix.drawPixel(x0_pos, i, matrix.Color333(color_coord[0],color_coord[1],color_coord[2]));
          if(!strcmp(color, "black"))
            delay(500);
         }
       } else { //qui invece in basso, le y decrescono
          int j;
          for (i = y1_pos; i >= y0_pos; i--) {
            matrix.drawPixel(x0_pos, i, matrix.Color333(color_coord[0],color_coord[1],color_coord[2]));
            if(!strcmp(color, "black"))
              delay(500);
          }
        }
     } else if(y0_pos == y1_pos) { //qui si muove invece solo la coordinata x
          if(x0_pos < x1_pos) {   //qui ci spostiamo a destra, le x crescono
            int i;
            for(i = x0_pos; i<=x1_pos; i++) {
             matrix.drawPixel(i, y0_pos, matrix.Color333(color_coord[0],color_coord[1],color_coord[2]));
             if(!strcmp(color, "black"))
              delay(500);        
            }
          } else {               //qui ci spostiamo a sinistra, le x decrescono
              int j;
              for (j = x1_pos; j >= x0_pos; i--) {
                matrix.drawPixel(i, y0_pos, matrix.Color333(color_coord[0],color_coord[1],color_coord[2]));
                if(!strcmp(color, "black"))
                  delay(500);
              }
            }
         } else if(x0_pos == x1_pos && y0_pos == y1_pos) { //caso degenere, solo un led deve essere acceso o spento
            matrix.drawPixel(x0_pos, y0_pos, matrix.Color333(color_coord[0],color_coord[1],color_coord[2]));
            if(!strcmp(color, "black"))
              delay(500);
            }
}//fine del loop
