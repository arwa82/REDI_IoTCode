#include <DHT.h>
#define DHTTYPE DHT11

//Set’s the pin we’re reading data from and initializes the sensor. 
int DHTPIN = 4;
DHT dht(DHTPIN,DHTTYPE);

void setup() {

//Tell the arduino we’ll be reading data on the defined DHT pin 
pinMode(DHTPIN, INPUT);
pinMode(LED_BUILTIN, OUTPUT);

//Open the serial port for communication
Serial.begin(9600); 
//start the connection for reading.
dht.begin();

}

void loop() {

digitalWrite(LED_BUILTIN, HIGH);

//declare variables for storing temperature and humidity and capture
float h = dht.readHumidity();

// Read temperature as Celsius
float t = dht.readTemperature();

//output data as humidity,temperature 
Serial.print(h);
Serial.print(",");
Serial.println(t);

//println includes linefeed  
//sleep for two seconds before reading again 

delay(500);
digitalWrite(LED_BUILTIN, LOW);
delay(2000); 

}
