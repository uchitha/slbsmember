--Payment Status
select m.MembershipNumber, m.ContactName,LEFT(CONVERT(VARCHAR, m.PaidUpTo, 120), 10) PaidUpTo from Membership m
where m.MembershipNumber not like 'X%'
order by m.MembershipNumber
