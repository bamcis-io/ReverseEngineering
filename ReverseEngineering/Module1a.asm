BITS 32

section .text
	push	ebp
	mov		ebp, esp
	lea		esi, [ string ]

string:
db	'hello world',0xd,0x0
end_string: