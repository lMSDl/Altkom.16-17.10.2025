CREATE VIEW View_OrderSummary
AS
	SELECT o.Id, o.[OrderDate], o.TotalValue
	FROM [Orders] AS o
	JOIN [Products] AS p ON o.Id = p.OrderId
	GROUP BY o.Id, o.[OrderDate], o.TotalValue
