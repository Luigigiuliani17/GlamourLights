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
int x_pos,y_pos;
int color_coord[3];

void setup() {

  matrix.begin();
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
x_pos = Serial.readStringUntil(':').toInt(); // Put the serial input into the message
Serial.read();
y_pos = Serial.readStringUntil(':').toInt();
Serial.read();
//colore e' una stringa la quale verra', della quale verra verificato il valore in if in cascata
color = Serial.readStringUntil(':');
Serial.read();

//switch che decide l'azione principale (spegnimento/accensione)

    //Qui si computano le coordinate colore a seconda delle direttive seriali
    //i colori "wall" e "shelf" saranno per i led che identificano scaffali e muri 
    //pertanto saranno sempre uguali e accesi sempre
    if(strcmp(color, "red"))
    color_coord = {5,0,0};  
    if(strcmp(color, "green"))
    color_coord = {0,5,0};
    if(strcmp(color, "blue"))
    color_coord = {0,0,5};
    if(strcmp(color, "yellow"))
    color_coord = {0,5,5};
    if(strcmp(color, "black"))
    color_coord = {0,0,0};
    if(strcmp(color, "wall"))
    color_coord = {1,1,1};
    if(strcmp(color, "shelf"))
    color_coord = {0,1,1}; 

          //gestione di accensione e spegnimento dei led (lo spegnimento sara' stantaneo a differenza dall'accensione)
          matrix.drawPixel(x_pos, y_pos, matrix.Color333(color_coord[0],color_coord[1],color_coord[2]));
          if(strcmp(color, "red")||strcmp(color, "green")||strcmp(color, "blue")||strcmp(color, "yellow"))
          delay(500);
         
}//fine del loop
