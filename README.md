# FPGA MD5 Cracker

This project is a hardware MD5 cracker built around a high-throughput, pipelined implementation of the MD5 hash function. It consists of three devices:

##### DE0-Nano FPGA
The workhorse. Other FPGAs will likely work, but this is the one I went with. The design is quite large, consuming 21,257/22,320 (95%) of the logical elements.

##### Netduino Plus 2
Acts as a programmer, interfacing with the FPGA design via SPI and the computer using Ethernet. I chose the NP2 because it was the only 3.3v SPI master device I had on hand. Others should work, but if you use a DE0-Nano, keep in mind the Cyclone IV is not 5v tolerant. It's also worth noting that, should one choose another device, they'll likely have to rewrite the programmer since the current implementation is written in C#.

##### Arduino LCD Shield
Displays the output of the cracker. Of course, this doesn't have to be an Arduino shield, but that's what I had. Any Hitachi HD44780 compatible LCD should work. To simplify wiring, the cracker uses the LCD in 4-bit mode.

### Circuit
| DE0-Nano | Netduino Plus2/LCD  |
| :------- | :------------------ |
| JP2 38   | Netduino Plus 2 D10 |
| JP2 39   | Netduino Plus 2 D11 |
| JP1 5    | Netduino Plus 2 D12 |
| JP2 40   | Netduino Plus 2 D13 |
| JP1 30   | Netduino Plus 2 Gnd |
| JP1 35   | LCD Shield D4       |
| JP1 36   | LCD Shield D5       |
| JP1 37   | LCD Shield D6       |
| JP1 38   | LCD Shield D7       |
| JP1 39   | LCD Shield D8       |
| JP1 40   | LCD Shield D9       |
| JP1 11   | LCD Shield 5v       |
| JP1 12   | LCD Shield Gnd      |

| Name     | Jumper | Node Name     | Location | Netduino Plus/2 LCD |
| :------- | :----- | :------------ | :------- | :------------------ |
| SS       | JP2 38 | GPIO\_1\[31\] | PIN\_K15 | Netduino Plus 2 D10 |
| MOSI     | JP2 39 | GPIO\_1\[32\] | PIN\_J13 | Netduino Plus 2 D11 |
| MISO     | JP1 5  | GPIO\[2\]     | PIN\_A2  | Netduino Plus 2 D12 |
| SCK      | JP2 40 | GPIO\_1\[33\] | PIN\_J14 | Netduino Plus 2 D13 |
| GND      | JP1 30 |               |          | Netduino Plus 2 Gnd |
| DB4      | JP1 35 | GPIO\[28\]    | PIN\_C11 | LCD Shield D4       |
| DB5      | JP1 36 | GPIO\[29\]    | PIN\_B11 | LCD Shield D5       |
| DB6      | JP1 37 | GPIO\[30\]    | PIN\_A12 | LCD Shield D6       |
| DB7      | JP1 38 | GPIO\[31\]    | PIN\_D11 | LCD Shield D7       |
| RS       | JP1 39 | GPIO\[32\]    | PIN\_D12 | LCD Shield D8       |
| ENABLE   | JP1 40 | GPIO\[33\]    | PIN\_B12 | LCD Shield D9       |
| VCC\_SYS | JP1 11 |               |          | LCD Shield 5v       |
| GND      | JP1 12 |               |          | LCD Shield Gnd      |

