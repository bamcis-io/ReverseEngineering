BITS 32

; Adds two values together and leaves them in eax
function:
	push	ebp								; Saves value of ebp to stack
	mov		ebp, esp						; Saves the location of ebp's previous value on the stack to ebp
	sub		esp, 8							; Move the stack pointer two words up

	mov		eax, dword [ ebp + 0x08 ]		; ebp points to the top of the stack, plus 4 points to return address, plus 8 points to first argument, load that into eax
	add		eax, dword [ ebp + 0x0c ]		; add the second argument, which is 4 more bytes down the stack than the first, to the value in eax and save it back there

	add		esp, 8							; move the stack pointer back down to where ebp is pointing
	pop		ebp								; move the value at the top of the stack into ebp, which is the original value of ebp
	retn	8								; EIP is set to the value at the top of the stack, the return address, and then the stack pointer is moved down 8 bytes, which is the location before the function was invoked, clears the arguments

push 8										; put 8 on the stack, second argument
push 4										; put 4 on the stack, first argument
call function								; call the function, i.e. function(4, 8)