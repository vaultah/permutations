My task was to make a C# program to print information about entered permutations, including

 - order
 - parity
 - signature
 - decomposition into disjoint cycles
 
It originally had a full-featured GUI (Windows Forms), which I decided to strip off. This repository is a VS solution,
consisting of 2 projects, one of which contains unit tests.



`Permutations.Permutation` and `Permutations.Cycle` represent permutations and permutations cycles, respectively.
They both have `from_string` methods that can be used to construct an instance of the respective class from a string representation of
a permutation or a cycle.

> ***Note:*** *the product `ab` represents the permutation `a(b(∙))`*

###String representation formats:

####Cycles
Any number of positive integers separated by whitespace characters. Each cycle must be enclosed with parentheses e.g.
```
(1 2 3)
```
####Permutations
A sequence of cycles, optionally followed by ` ^ <power>`. If ` ^ <power>` is present, cycles must be wrapped with 
square brackets.

```
  (1 2 3)(4 5 6) // valid
  (1 2 3)(4 5 6) ^ 7 // not valid
  [(1 2 3)(4 5 6)] ^ 7 // valid
```

###Command line interface

Displays properties of entered permutations. If the input is empty, it will a generate a random permutation. Example:

![](http://i.imgur.com/XMfCMcd.png)

Wolfram Alpha results for permutations above:
[1](https://www.wolframalpha.com/input/?i=permutation+((2+9+4+7)(5+4+2+7)(7+8+5)(2+5+3))+%5E+307),
[2](https://www.wolframalpha.com/input/?i=permutation+(1)),
[3](https://www.wolframalpha.com/input/?i=permutation+((8+6+1)(1+2+3+4)(2+8+1+5+9)(1+8+9))+%5E+9774).
