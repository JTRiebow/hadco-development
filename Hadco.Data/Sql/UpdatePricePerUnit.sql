with T as
(
select lt.LoadTimerID, lt.PricePerUnit, pr.[Value]
from LoadTimers lt
cross apply (select top 1 StartTime 
			from LoadTimerEntries lte 
			where lt.LoadTimerID = lte.LoadTimerID 
			order by StartTime) lte
join Jobs j on lt.JobID = j.JobID
cross apply(select top 1 * from Pricings p 
			where lt.BillTypeID = p.BillTypeID and j.CustomerTypeID = p.CustomerTypeID 
			and (lt.JobID = p.JobID or j.CustomerID = p.CustomerID or lt.PhaseID = p.PhaseID)
			and p.StartDate <= lte.StartTime and (p.EndDate is null or lte.StartTime <= p.EndDate)
			order by p.UpdatedTime desc) as p
join Prices pr on p.PricingID = pr.PricingID 
	and (lt.TruckClassificationID = pr.TruckClassificationID or lt.MaterialID = pr.MaterialID)
where (p.JobID = @jobID or p.CustomerID = @customerID or p.PhaseID = @phaseID)
and p.BillTypeID = @billTypeID
and p.CustomerTypeID = @customerTypeID
and (pr.MaterialID = @materialID or pr.TruckClassificationID = @truckClassificationID)
)
update T
set PricePerUnit = [Value]