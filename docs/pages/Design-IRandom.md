# IRandom

The IRandom classes provide primitive value type randomization without reflection. There are three default implementations:

* FastRandom - Uses a base that is quick but not cryptographically secure.
* SecureRandom - Uses a base that is cryptographically secure but slow.
* SeededRandom - Generates deterministic values.

By default only valid values are generated (not NaN or infinities), but that can be changed through the constructor.

Providing an initial seed and using the SeededRandom enables one to get the same random numbers generated between runs. This is useful for generating things that appear random, but are the same for all users for example.
