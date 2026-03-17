import random
from typing import TypeVar, Generic, List

T = TypeVar('T')

class RandomizedList(Generic[T]):
    def __init__(self):
        self._items: List[T] = []

    def is_empty(self) -> bool:
        return len(self._items) == 0

    def add(self, element: T) -> None:
        if random.random() >= 0.5:
            self._items.append(element)
        else:
            self._items.insert(0, element)

    def get(self, max_index: int) -> T:
        if self.is_empty():
            raise ValueError("The collection is empty.")

        safe_max = max(0, min(max_index, len(self._items) - 1))

        random_index = random.randint(0, safe_max)

        return self._items[random_index]

if __name__ == "__main__":
    my_list = RandomizedList[str]()
    print(f"Is empty? {my_list.is_empty()}")

    for item in ["1", "2", "3", "4", "5"]:
        my_list.add(item)

    print(f"Internal state: {my_list._items}")
    print(f"Random Get (up to index 3): {my_list.get(3)}")