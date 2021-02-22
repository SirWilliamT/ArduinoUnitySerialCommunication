void setup()
{
    // Set your baud rate for your Serial connection
    Serial.begin(9600);

    // Wait for the serial port to connect.
    while (!Serial) {}
}

void loop()
{
    if (Serial.available() > 0)
    {
        // Got something
        int read = Serial.read();
        if (read >= 0)
        {
            Serial.write(read);
        }
    }
}
