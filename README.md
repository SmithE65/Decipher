Decipher

Attempts to decipher simple, single substitution ciphers by pattern matching against a lit of known words.

The current implementation is very much code vomit and proof of concept.  Next steps include factoring in word frequency, testing for articles and short, common words first, and then an architectural refactoring. 

Console app takes two files as arguments
1) a word list w/ line separated words (ex. https://www.mit.edu/~ecprice/wordlist.10000 or there are some 500k+ lists on github)
2) a text file with the ciphered text

