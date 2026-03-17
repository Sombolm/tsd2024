import requests
import pandas as pd
from datetime import datetime, timedelta

class GoldDataService:
    BASE_URL = "http://api.nbp.pl/api/cenyzlota/"

    def get_gold_prices(self, start_date: datetime, end_date: datetime) -> pd.DataFrame:
        all_prices = []
        current_start = start_date

        print(f"Fetching data from NBP API: {start_date.date()} to {end_date.date()}...")

        while current_start <= end_date:
            current_end = current_start + timedelta(days=92)
            if current_end > end_date:
                current_end = end_date

            start_str = current_start.strftime("%Y-%m-%d")
            end_str = current_end.strftime("%Y-%m-%d")

            url = f"{self.BASE_URL}{start_str}/{end_str}"
            response = requests.get(url, headers={'Accept': 'application/json'})

            if response.status_code == 200:
                data = response.json()
                for item in data:
                    all_prices.append({
                        "Date": pd.to_datetime(item["data"]),
                        "Price": item["cena"]
                    })

            current_start = current_end + timedelta(days=1)

        return pd.DataFrame(all_prices)

if __name__ == "__main__":
    service = GoldDataService()

    end_date = datetime.now()
    start_date = end_date - timedelta(days=365)

    df = service.get_gold_prices(start_date, end_date)

    if df.empty:
        print("No data retrieved.")
    else:
        print(f"Retrieved {len(df)} records. Ready for analysis!\n")

        print("=== TOP 3 Highest Prices ===")
        top_3_highest = df.nlargest(3, 'Price')

        print(top_3_highest['Date'].dt.strftime('%Y-%m-%d').astype(str) + " - " +
              top_3_highest['Price'].astype(str) + " PLN")

        print("\n=== TOP 3 Lowest Prices ===")
        top_3_lowest = df.nsmallest(3, 'Price')

        print(top_3_lowest['Date'].dt.strftime('%Y-%m-%d').astype(str) + " - " +
              top_3_lowest['Price'].astype(str) + " PLN")