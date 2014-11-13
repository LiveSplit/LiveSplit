(define (cadr lst) 
  (car (cdr lst)))

(define mylist (list 1 "foo" 3))

(display "result: " (cadr mylist))
