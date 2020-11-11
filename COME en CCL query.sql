select cast(round(p.ClosingPrice/d.Price,2) as numeric(36,2)) from PriceData p 
join Symbol s on p.Symbol_ID = s.ID 
join DollarData d on p.Date = d.ExchangeDate
where s.Name = 'COME' order by p.Date