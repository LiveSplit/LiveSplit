(define (fib n)
  (if (< n 2) 
     1 
     (+ (fib (- n 1)) (fib (- n 2)))))
	 
(display (fib 20)) ; fib(20)= 10946
