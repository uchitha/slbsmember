select count(*) from Membership


order by MembershipNumber

select m.MembershipNumber, c.FullName,c.ClassLevel,t1.FullName from Child c
inner join Membership m on m.MembershipId = c.MembershipId
inner join (select m.MembershipId, m.MembershipNumber, a.FullName from Adult a
inner join Membership m on a.MembershipId = m.MembershipId
where email is null) t1 on t1.MembershipId = m.MembershipId
order by ClassLevel


select m.MembershipNumber, a.FullName as MemberName, a.MobilePhone, c.ClassLevel as Class, c.FullName as ChildName from Adult a
inner join Membership m on a.MembershipId = m.MembershipId
left outer join Child c on c.MembershipId = m.MembershipId
where email is null
order by Class desc


select MembershipNumber, ContactName
from Membership 
where MembershipNumber not like 'X%'
order by MembershipNumber

inner join Adult on Membership.MembershipId = Adult.MembershipId