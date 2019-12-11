select top 1 tc.TruckClassificationID
from TruckClassifications tc
join Equipment truck on @TruckID = truck.EquipmentID
left join Equipment trailer on @TrailerID = trailer.EquipmentID
left join Equipment pup on @PupID = pup.EquipmentID
where (truck.EquipmentNumber like tc.Truck + '%' or truck.EquipmentNumber like 'S%')
and (trailer.EquipmentNumber like tc.Trailer1 + '%' or tc.Trailer1 is null)
and (pup.EquipmentNumber like tc.Trailer2 + '%' or tc.Trailer2 is null)
order by tc.Trailer2 desc, tc.Trailer1 desc
