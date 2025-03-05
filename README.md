# Beat Mapper
Free to use however you please. The waveform visualization is wrong, and the scrollbar doesn't properly track your location. If you set the offset too high, the beginning/end of the song will be inaccessible (though you can just manually modify this in the output file).

## Usage

This program expects an audio file in and optionally a `beatmap` file. It can have any extension, so long as the content is raw text. The format of a `beatmap` file looks like the following:

```
bpm,offset
map1
map2
map3
...
```
So for a bpm of `120`, an offset of `-0.02` and 4 maps, the file would look like the following:
```
120,-0.02
11111111
01010101
00010001
10101010
```

## Usage with [UltraBeat](https://thunderstore.io/c/ultrakill/p/Recessive/UltraBeat/) format
I initially made this in a couple of weeks just to help me quickly map songs for my own rhythm games. As such, it's pretty barebones and the format is less than ideal. However because the format is incredibly simple, it is ridiculously easy to translate over to `json`.

[UltraBeat](https://thunderstore.io/c/ultrakill/p/Recessive/UltraBeat/) expects a `json` file of the following format:

```json
{
  "offset": 0,
  "bpm": 120,
  "maps": {
    "base": [1,1,1,1,1,1,1,1,1],
    "freeze_short": [1,0,1,0,1,0,1,0,1],
    "freeze_long": [1,0,0,0,1,0,0,0,1],
    "fast": [1,0,1,0,0,0,1,0,1]
  }
}
```
This would look like the following in `beatmap` format:
```
120,0
111111111
101010101
100010001
101010101
```
As the beat maps get longer, you'll probably want to avoid manually converting a string into a list. To save time you can use a simple python command to quickly convert between the two. For example, to convert `111111111` to `[1,1,1,1,1,1,1,1,1]`:
```python3
a = "111111111"
print([int(i) for i in a])
> [1, 1, 1, 1, 1, 1, 1, 1, 1]
```
And this will output the string as a list for you to copy into the `json`

### ![#f03c15](https://placehold.co/15x15/f03c15/f03c15.png) Please note, this software and UltraBeat have very different offsets. I had to set my offet to `-0.95` in BeatMapper and `0` in UltraBeat in order for the beats to line up

