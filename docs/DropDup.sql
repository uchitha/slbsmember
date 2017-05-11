select count(*) as AdultsCount,MembershipId from Adult group by MembershipId having count(*) > 2

delete from Adult
where 
AdultId in (
select adultId from Adult
where MembershipId in (select MembershipId from Adult group by MembershipId having count(*) > 2)
and AdultId > 1000
)

