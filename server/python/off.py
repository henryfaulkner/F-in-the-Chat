import board
import neopixel
from time import sleep

pixels = neopixel.NeoPixel(board.D10, 50, brightness=1)
# green, red, blue
pixels.fill((0,0,0))
pixels.show()
